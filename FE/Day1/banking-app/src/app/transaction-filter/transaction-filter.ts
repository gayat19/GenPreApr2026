import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TransactionFilter } from '../models/transaction.filter.model';
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
    console.log('Filter updated:', this.filter());
    this.store.dispatch(
        updateTransactionFilter({ filter: this.filter() })
      )
  }

  updateFromAccountNumber(value: string) {
    this.filter.update((current) => ({
      ...current,
      fromAccountNumber: value,
    }));
    this.onSearchChange();
  }

  updateMinAmount(value: number | null) {
    this.filter.update((current) => ({
      ...current,
      minAmount: value ?? undefined,
    }));
    this.onSearchChange();
  }

  updateMaxAmount(value: number | null) {
    this.filter.update((current) => ({
      ...current,
      maxAmount: value ?? undefined,
    }));
    this.onSearchChange();
  }



  onClearFilter(){
    this.filter.set(new TransactionFilter());
    this.store.dispatch(
        updateTransactionFilter({ filter: this.filter() })
      )
  }
}
