import { Routes } from '@angular/router';
import { Customers } from './customers/customers';
import { Account } from './account/account';
import { Transaction } from './transaction/transaction';
import { Login } from './login/login';
import { authGuard } from './guards/authGuard';
import { TransactionHome } from './transaction-home/transaction-home';


export const routes: Routes = [
     {path:'',component:TransactionHome},
    {path:'login',component:Login},
    {path:'home',component:Customers},
   
    {path:'account',component:Account,
        canActivate:[authGuard],
        children:[
        // {path:'transaction/:accNum',component:Transaction}
        {path:'transaction',component:Transaction}
    ]},
    
    {path:'products',loadComponent:()=>import('./products/products').then(m=>m.Products)},
    

];
