import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TransactionFilter } from '../models/transaction.filter.model';
import { FormField } from "@angular/forms/signals";
import { Store } from '@ngrx/store';
import { updateTransactionFilter } from '../store/transaction.actions';

@Component({
  selector: 'app-transaction-filter',
  imports: [FormsModule],
  templateUrl: './transaction-filter.html',
  styleUrl: './transaction-filter.css',
})
export class TransactionFilterComponent {
  filter = signal(new TransactionFilter());

  constructor(private store:Store){
      this.store.dispatch(
        updateTransactionFilter({ filter: this.filter() })
      )
  }

  onSearchChange(){
    this.store.dispatch(
        updateTransactionFilter({ filter: this.filter() })
      )
  }



  onClearFilter(){
    this.filter.set(new TransactionFilter());
    this.store.dispatch(
        updateTransactionFilter({ filter: this.filter() })
      )
  }
}
