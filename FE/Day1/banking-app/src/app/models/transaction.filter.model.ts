export class TransactionFilter {
    fromAccountNumber:string|undefined;
    minAmount:number|undefined;
    maxAmount:number|undefined;
    fromDate:Date|undefined;
    toDate:Date|undefined;
    pageNumber:number = 1;
    pageSize:number= 10;  
    sortBy:string = "TransactionDate";
    sortDirection:string = "desc";
}