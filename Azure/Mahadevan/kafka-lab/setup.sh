#!/bin/bash
set -e
exec > /var/log/kafka-setup.log 2>&1
 
echo "=== Kafka Lab Setup Started ==="
date
 
# 1. System packages
apt-get update -y
apt-get install -y openjdk-17-jdk wget curl
 
# 2. Set JAVA_HOME system-wide
echo "JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64" >> /etc/environment
export JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64
 
# 3. Install .NET 8 SDK
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb \
     -O /tmp/ms-prod.deb
dpkg -i /tmp/ms-prod.deb
apt-get update -y
apt-get install -y dotnet-sdk-8.0
 
# 4. Download and extract Kafka 3.7.0
cd /opt
wget https://archive.apache.org/dist/kafka/3.7.0/kafka_2.13-3.7.0.tgz -O kafka.tgz
tar -xzf kafka.tgz
mv kafka_2.13-3.7.0 kafka
rm kafka.tgz
 
# 5. Format KRaft storage (single-node, no ZooKeeper)
KAFKA_UUID=$(JAVA_HOME=$JAVA_HOME /opt/kafka/bin/kafka-storage.sh random-uuid)
echo "Kafka cluster UUID: $KAFKA_UUID"
JAVA_HOME=$JAVA_HOME /opt/kafka/bin/kafka-storage.sh format \
  -t "$KAFKA_UUID" \
  -c /opt/kafka/config/kraft/server.properties
 
# 6. Create systemd service
cat > /etc/systemd/system/kafka.service << EOF
[Unit]
Description=Apache Kafka (KRaft Mode)
After=network.target
 
[Service]
Type=simple
User=root
Environment=JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64
ExecStart=/opt/kafka/bin/kafka-server-start.sh /opt/kafka/config/kraft/server.properties
ExecStop=/opt/kafka/bin/kafka-server-stop.sh
Restart=on-failure
RestartSec=10
 
[Install]
WantedBy=multi-user.target
EOF
 
systemctl daemon-reload
systemctl enable kafka
systemctl start kafka
 
# 7. Wait for Kafka to be ready, then create the topic
MAX_WAIT=90; WAITED=0
until JAVA_HOME=$JAVA_HOME /opt/kafka/bin/kafka-topics.sh \
    --list --bootstrap-server localhost:9092 > /dev/null 2>&1; do
  [ $WAITED -ge $MAX_WAIT ] && echo "ERROR: Kafka timeout" && exit 1
  echo "Waiting for Kafka... (${WAITED}s elapsed)"
  sleep 5; WAITED=$((WAITED+5))
done
 
JAVA_HOME=$JAVA_HOME /opt/kafka/bin/kafka-topics.sh \
  --create --topic payment-events \
  --bootstrap-server localhost:9092 \
  --partitions 1 --replication-factor 1
 
echo "=== Setup Complete ===" && date
