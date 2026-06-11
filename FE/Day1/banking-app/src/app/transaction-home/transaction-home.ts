import { Component } from '@angular/core';

import {  TransactionList } from "../transaction-list/transaction-list";
import {  TransactionFilterComponent } from '../transaction-filter/transaction-filter';

@Component({
  selector: 'app-transaction-home',
  imports: [TransactionFilterComponent, TransactionList],
  templateUrl: './transaction-home.html',
  styleUrl: './transaction-home.css',
})
export class TransactionHome {}
