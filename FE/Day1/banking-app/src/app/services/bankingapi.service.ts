import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { LoginModel } from "../models/login.model";
import { baseUrl } from "../../enironment";

@Injectable({
    providedIn: 'root'
})
export class BankingApiService {
    constructor(private http: HttpClient) {
    }
    public loginApiCall(loginModel: LoginModel) {
        let url = baseUrl+'/Authentication/Login';
        return this.http.post(url, loginModel);
    }
}