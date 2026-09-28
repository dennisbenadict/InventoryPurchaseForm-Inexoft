import { Component, OnInit } from '@angular/core';
import { PurchaseService } from '../../core/services/purchase.service';
import { Purchase } from '../../core/models/models';
import { Router } from '@angular/router';

@Component({
  selector: 'app-purchases',
  templateUrl: './purchases.component.html',
  styleUrls: ['./purchases.component.scss']
})
export class PurchasesComponent implements OnInit {
  purchases: Purchase[] = [];
  displayedColumns: string[] = ['invoiceNumber', 'invoiceDate', 'supplierName', 'netAmount', 'status', 'actions'];

  constructor(private purchaseService: PurchaseService, private router: Router) {}

  ngOnInit(): void {
    this.loadPurchases();
  }

  loadPurchases(): void {
    this.purchaseService.getAll().subscribe(data => {
      this.purchases = data;
    });
  }

  navigateToCreate(): void {
    this.router.navigate(['/purchases/new']);
  }

  editPurchase(id: number): void {
    this.router.navigate(['/purchases', id]);
  }

  postPurchase(id: number): void {
    this.purchaseService.post(id).subscribe(() => {
      this.loadPurchases();
    });
  }
}
