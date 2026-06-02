export class CustomerModel
{


    constructor(public username: string = "", public name: string = "", public email: string = "", public phone: string = "", public status: string = "", public dateOfBirth: Date = new Date())
    {
        
    }
}