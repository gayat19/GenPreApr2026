param location   string = 'eastus'
param vmName     string = 'kafka-lab-vm'
param adminUsername string = 'azureuser'
 
@secure()
param adminPassword string
 
param vmSize string = 'Standard_B2ms'
 
var nsgName      = '${vmName}-nsg'
var vnetName     = '${vmName}-vnet'
var subnetName   = 'default'
var publicIpName = '${vmName}-pip'
var nicName      = '${vmName}-nic'
 
resource nsg 'Microsoft.Network/networkSecurityGroups@2023-04-01' = {
  name: nsgName
  location: location
  properties: {
    securityRules: [{
      name: 'AllowSSH'
      properties: {
        priority: 1000
        protocol: 'Tcp'
        access: 'Allow'
        direction: 'Inbound'
        sourceAddressPrefix: '*'
        sourcePortRange: '*'
        destinationAddressPrefix: '*'
        destinationPortRange: '22'
      }
    }]
  }
}
 
resource vnet 'Microsoft.Network/virtualNetworks@2023-04-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: { addressPrefixes: ['10.0.0.0/16'] }
    subnets: [{
      name: subnetName
      properties: { addressPrefix: '10.0.0.0/24', networkSecurityGroup: { id: nsg.id } }
    }]
  }
}
 
resource publicIp 'Microsoft.Network/publicIPAddresses@2023-04-01' = {
  name: publicIpName
  location: location
  sku: { name: 'Standard' }
  properties: { publicIPAllocationMethod: 'Static' }
}
 
resource nic 'Microsoft.Network/networkInterfaces@2023-04-01' = {
  name: nicName
  location: location
  properties: {
    ipConfigurations: [{
      name: 'ipconfig1'
      properties: {
        subnet:          { id: '${vnet.id}/subnets/${subnetName}' }
        publicIPAddress: { id: publicIp.id }
      }
    }]
  }
}
 
resource vm 'Microsoft.Compute/virtualMachines@2023-07-01' = {
  name: vmName
  location: location
  properties: {
    hardwareProfile: { vmSize: vmSize }
    osProfile: {
      computerName:   vmName
      adminUsername:  adminUsername
      adminPassword:  adminPassword
    }
    storageProfile: {
      imageReference: {
        publisher: 'Canonical'
        offer:     '0001-com-ubuntu-server-jammy'
        sku:       '22_04-lts-gen2'
        version:   'latest'
      }
      osDisk: {
        createOption: 'FromImage'
        managedDisk:  { storageAccountType: 'StandardSSD_LRS' }
      }
    }
    networkProfile: {
      networkInterfaces: [{ id: nic.id }]
    }
  }
}
 
resource setup 'Microsoft.Compute/virtualMachines/extensions@2023-07-01' = {
  parent: vm
  name: 'setup-kafka'
  location: location
  properties: {
    publisher:            'Microsoft.Azure.Extensions'
    type:                 'CustomScript'
    typeHandlerVersion:   '2.1'
    autoUpgradeMinorVersion: true
    settings: {}
    protectedSettings: {
      script: base64(loadTextContent('setup.sh'))
    }
  }
}
 
output publicIpAddress string = publicIp.properties.ipAddress
output sshCommand      string = 'ssh ${adminUsername}@${publicIp.properties.ipAddress}'
