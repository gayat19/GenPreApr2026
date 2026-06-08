import { Component, input, Input } from '@angular/core';
import { ProductModel } from '../models/product.model';

@Component({
  selector: 'app-product',
  imports: [],
  templateUrl: './product.html',
  styleUrl: './product.css',
})
export class Product {
  //@Input() product:ProductModel = {} as ProductModel;
  product = input<ProductModel>();
}
