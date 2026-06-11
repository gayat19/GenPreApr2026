export class TransactionFilter {
    public fromAccountNumber:string|undefined;
    public minAmount:number|undefined;
    public maxAmount:number|undefined;
    public fromDate:Date|undefined;
    public toDate:Date|undefined;
    public pageNumber:number = 1;
    public pageSize:number= 10;  
    public sortBy:string = "TransactionDate";
    public sortDirection:string = "desc";
}