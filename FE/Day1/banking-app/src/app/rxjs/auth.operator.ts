import { Subject } from "rxjs";

export const usernameSubject = new Subject<string|undefined>();

export const logout = () => {
    sessionStorage.removeItem("token");
    usernameSubject.next(undefined);
}

export const isLoggedIn = () => {
    const token = sessionStorage.getItem("token");
    return token?true:false;
}

export const changeUsername = () => {
    // console.log("Changing username to", username);
    const token = sessionStorage.getItem("token");
    if (token) {
        const payload = JSON.parse(atob(token.split(".")[1]));
         const name = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"];
         if (name) {
            usernameSubject.next(name);
         }
    }
    
}