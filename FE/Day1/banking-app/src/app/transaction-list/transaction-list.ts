import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { selectTransactionError, selectTransactionList, selectTransactionLoading } from '../store/transaction.selector';

import { loadTransaction } from '../store/transaction.actions';
import { AsyncPipe, DatePipe } from '@angular/common';

@Component({
  selector: 'app-transaction-list',
  imports: [AsyncPipe,DatePipe],
  templateUrl: './transaction-list.html',
  styleUrl: './transaction-list.css',
})
export class TransactionList implements OnInit {
  transactions$: Observable<any> | undefined;
  loading$: Observable<any> | undefined;
  error$: Observable<any> | undefined;

  

  constructor(private store:Store){}

  ngOnInit(): void {
    this.transactions$ = this.store.select(selectTransactionList);
    this.loading$ = this.store.select(selectTransactionLoading);
    this.error$ = this.store.select(selectTransactionError);

    this.store.dispatch(loadTransaction());
    // this.transactions$.subscribe((data) => {      
    //   console.log('Transactions:', data);
    // });
  }
}
