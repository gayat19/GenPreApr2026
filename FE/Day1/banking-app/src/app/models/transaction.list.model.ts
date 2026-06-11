import { Transaction } from "./transaction.model";

export interface TransactionList {
    items: Transaction[];
    pageNumber: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
}
