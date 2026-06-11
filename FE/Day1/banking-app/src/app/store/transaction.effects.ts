import { inject, Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from "@ngrx/effects";
import { clearTransactionFilter, loadTransaction, loadTransactionFailure, 
    loadTransactionSuccess, 
    updateTransactionFilter} from "./transaction.actions";
import { catchError, debounceTime, map, of, switchMap, withLatestFrom } from "rxjs";
import { Store } from "@ngrx/store";
import { slectTransactionFilter } from "./transaction.selector";
import { TransactionService } from "../services/transaction.service";


@Injectable()
export class TransactionEffects {
    private actions$ = inject(Actions);
    private store = inject(Store);
    private transactionService = inject(TransactionService);

loadTransactionListOnFilterChange$ = createEffect(()=>{
    return this.actions$.pipe(
        ofType(updateTransactionFilter,clearTransactionFilter),
        debounceTime(500),
        map(()=>loadTransaction()))
});

    loadTransaction$ = createEffect(()=>{
        return this.actions$.pipe(
            ofType(loadTransaction),
            withLatestFrom(this.store.select(slectTransactionFilter)),
            switchMap(([action, filter])=>
                this.transactionService.getTransactions(filter).pipe(
                    map(transactionList=>loadTransactionSuccess({transactionList} as any)),
                    catchError(error=>of(loadTransactionFailure({error: error.message})))
                )
            )
        )
    });
}