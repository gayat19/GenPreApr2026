import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-transaction',
  imports: [],
  templateUrl: './transaction.html',
  styleUrl: './transaction.css',
})
export class Transaction {
  fromaccountNumber: string = '';
  constructor(private activeRoute:ActivatedRoute){
   //this.fromaccountNumber = this.activeRoute.snapshot.params['accNum'];
   this.fromaccountNumber = history.state.accNum || '';
  } 
}
