export class Transaction {
    transactionReferenceNumber: number=0;
    transactionDate: Date = new Date();
    fromAccountNumber: string |undefined;
    toAccountNumber: string |undefined;
    amount: number = 0;
    status: string |undefined;
}