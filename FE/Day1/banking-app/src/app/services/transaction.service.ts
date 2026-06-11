import { Injectable } from "@angular/core";
import { baseUrl } from "../../enironment";
import { HttpClient } from "@angular/common/http";
import { TransactionFilter } from "../models/transaction.filter.model";


@Injectable({
    providedIn: 'root'})
export class TransactionService{
    private baseurl = baseUrl;
    constructor(private http: HttpClient) {}

    getTransactions(filter: TransactionFilter) {
        let url = this.baseurl+'/Banking/transactions/query';
        return this.http.post(url, filter);
    }

}