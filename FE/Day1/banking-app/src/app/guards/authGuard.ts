import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { isLoggedIn } from "../rxjs/auth.operator";


export const authGuard:CanActivateFn = () => {
    const router = inject(Router);

    const userStatus = isLoggedIn();
    if (userStatus) 
        return true;
    router.navigate(["/"]);
    return false;
}