import { createAction, props } from "@ngrx/store";
import { TransactionFilter } from "../models/transaction.filter.model";
import { TransactionList } from "../models/transaction.list.model";

export const updateTransactionFilter = createAction(
  "[TransactionList] Update Filter",
  props<{ filter: TransactionFilter }>()
);


export const clearTransactionFilter = createAction(
  "[TransactionList] Clear Filter"
);

export const loadTransaction = createAction(
  "[TransactionList] Load"
);

export const loadTransactionSuccess = createAction(
  "[TransactionList] Load Success",
    props<{ transactionList: TransactionList }>()
);

export const loadTransactionFailure = createAction(
  "[TransactionList] Load Failure",
    props<{ error: string }>()
);