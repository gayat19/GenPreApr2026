import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CustomerModel } from '../models/customer.model';

@Component({
  selector: 'app-customers',
  imports: [FormsModule],
  templateUrl: './customers.html',
  styleUrl: './customers.css',
})
export class Customers {

  //customer:CustomerModel = new CustomerModel("johndoe", "John Doe", "john.doe@example.com", "123-456-7890", "active", new Date());
  customer:CustomerModel = new CustomerModel();
  styclass: string = "bi bi-balloon-heart-fill";

  handleChangeClick(){
    this.customer.name = "Jane Doe";
    alert("Customer Name: " + this.customer.name);
  }


  toggleLike(){
    if(this.styclass === "bi bi-balloon-heart-fill"){
      this.styclass = "bi bi-balloon-heart";
    } else {
      this.styclass = "bi bi-balloon-heart-fill";
    }
  }
}
