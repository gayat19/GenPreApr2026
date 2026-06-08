import { Component, signal } from '@angular/core';
import { ProductModel } from '../models/product.model';
import { ProductApiService } from '../services/product.api.service';
import { Product } from "../product/product";

@Component({
  selector: 'app-products',
  imports: [Product],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class Products {
  products = signal<ProductModel[]>([]);

  constructor(private productApiService: ProductApiService) {
    this.productApiService.getProductsFromDummyJson()
      .subscribe({
      next:(response: any) => {
        this.products.set(response.products);
      },
      error:(error) => {
        console.error(error);
      },
      complete:()=>{
        
      }
    });
  }

  handleChangeClick(){
    
  }
}
