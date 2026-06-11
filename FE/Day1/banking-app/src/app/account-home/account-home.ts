import { Component } from '@angular/core';
import { AccountFilter } from "../account-filter/account-filter";
import { AccountList } from "../account-list/account-list";

@Component({
  selector: 'app-account-home',
  imports: [AccountFilter,  AccountList],
  templateUrl: './account-home.html',
  styleUrl: './account-home.css',
})
export class AccountHome {}
