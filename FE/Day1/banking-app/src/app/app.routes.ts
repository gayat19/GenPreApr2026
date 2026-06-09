import { Routes } from '@angular/router';
import { Customers } from './customers/customers';

export const routes: Routes = [
    {path:'home',component:Customers},
    {path:'products',loadComponent:()=>import('./products/products').then(m=>m.Products)},
];
