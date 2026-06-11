import { ApplicationConfig, provideBrowserGlobalErrorListeners, isDevMode } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ProductApiService } from './services/product.api.service';
import { BankingApiService } from './services/bankingapi.service';
import { authInterceptor } from './authInterceptor';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { TransactionService } from './services/transaction.service';
import { transactionReducer } from './store/transaction.reducer';
import { TransactionEffects } from './store/transaction.effects';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    ProductApiService,
    BankingApiService,
    TransactionService,
    provideStore({ transaction: transactionReducer }),
    provideEffects(TransactionEffects),//Forgot to add this line, which is required to register the effects class with the NgRx EffectsModule. This allows the effects defined in the TransactionEffects class to be properly initialized and used within the application.
    provideStoreDevtools({ maxAge: 25, logOnly: !isDevMode() }),
  ],
};
