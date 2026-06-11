import { createFeatureSelector, createSelector } from "@ngrx/store";
import { TransactionState } from "./transaction.reducer";


export const selectTransactionState  = createFeatureSelector<TransactionState>('transaction');

export const slectTransactionFilter = createSelector(
    selectTransactionState,
    (state) => state.filter
);

export const selectTransactionList = createSelector(
    selectTransactionState,
    (state) => state.transactionList
);

export const selectTransactionLoading = createSelector(
    selectTransactionState,
    (state) => state.loading
);

export const selectTransactionError = createSelector(
    selectTransactionState,
    (state) => state.error
);