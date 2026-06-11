import { createReducer, on } from "@ngrx/store";
import { TransactionFilter } from "../models/transaction.filter.model";
import { TransactionList } from "../models/transaction.list.model";
import { clearTransactionFilter, loadTransaction, loadTransactionFailure, loadTransactionSuccess, updateTransactionFilter } from "./transaction.actions";



export interface TransactionState{
    filter:TransactionFilter;
    transactionList: TransactionList | null;
    loading: boolean;
    error: string | null;
}

export const initialTransactionFilter: TransactionFilter = {
    fromAccountNumber: '',
    minAmount: 0,
    maxAmount: 0,
    fromDate: undefined,
    toDate: undefined,
    pageNumber: 1,
    pageSize: 10,
    sortBy: "TransactionDate",
    sortDirection: "desc"
};


export const initialTransactionState: TransactionState = {
    filter: initialTransactionFilter,
    transactionList: null,
    loading: false,
    error: null
};

export const transactionReducer = createReducer(
    initialTransactionState,

    on(updateTransactionFilter, (state, { filter }) => ({
        ...state,
        filter: { ...state.filter, ...filter },
    })),

    on(clearTransactionFilter, (state) => ({
        ...state,
        filter: initialTransactionFilter,
    })),

    on(loadTransaction, (state) => ({
        ...state,
        loading: true,
        error: null
    })),

    on(loadTransactionSuccess, (state, { transactionList }) => ({
        ...state,
        transactionList,
        loading: false,
        error: null
    })),

    on(loadTransactionFailure, (state, { error }) => ({
        ...state,
        transactionList: null,
        loading: false,
        error
    })) 
);
