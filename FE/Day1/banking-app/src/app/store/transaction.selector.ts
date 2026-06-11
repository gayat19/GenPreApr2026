import { createFeatureSelector, createSelector } from "@ngrx/store";
import { initialTransactionState, TransactionState } from "./transaction.reducer";


export const selectTransactionState  = createFeatureSelector<TransactionState>('transaction');

export const slectTransactionFilter = createSelector(
    selectTransactionState,
    (state) => state?.filter ?? initialTransactionState.filter
);

export const selectTransactionList = createSelector(
    selectTransactionState,
    (state) => state?.transactionList ?? null
);

export const selectTransactionLoading = createSelector(
    selectTransactionState,
    (state) => state?.loading ?? false
);

export const selectTransactionError = createSelector(
    selectTransactionState,
    (state) => state?.error ?? null
);