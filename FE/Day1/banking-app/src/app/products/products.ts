import { Component, signal } from '@angular/core';
import { ProductModel } from '../models/product.model';
import { ProductApiService } from '../services/product.api.service';

@Component({
  selector: 'app-products',
  imports: [],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class Products {
  product = signal(new ProductModel());

  constructor(private productApiService: ProductApiService) {
    this.productApiService.getProductsFromDummyJson()
      .subscribe({
      next:(response: any) => {
        console.log(response.products[0]);
        this.product.set(response.products[0]);
      },
      error:(error) => {
        console.error(error);
      },
      complete:()=>{
        console.log("Request completed");
      }
    });
  }

  handleChangeClick(){
    this.product().title = "New Product Name";
  }
}
