import { Subject } from "rxjs";

export const usernameSubject = new Subject<string>();


export const changeUsername = (username: string) => {
    console.log("Changing username to", username);
    usernameSubject.next(username);
}