import { createAction } from "@ngrx/store";
import { TransactionFilter } from "../models/transaction.filter.model";
import { TransactionList } from "../models/transaction.list.model";

export const updateTransactionFilter = createAction(
  "[TransactionList] Update Filter",
  (filter: TransactionFilter) => ({ filter })
);


export const clearTransactionFilter = createAction(
  "[TransactionList] Clear Filter"
);

export const loadTransactionSuccess = createAction(
  "[TransactionList] Load Success",
    (transactionList: TransactionList) => ({ transactionList })
);

export const loadTransactionFailure = createAction(
  "[TransactionList] Load Failure",
    (error: string) => ({ error })
);