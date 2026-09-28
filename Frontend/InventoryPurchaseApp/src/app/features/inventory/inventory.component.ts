import { Component, OnInit } from '@angular/core';
import { InventoryService } from '../../core/services/inventory.service';
import { InventoryItem } from '../../core/models/models';

@Component({
  selector: 'app-inventory',
  templateUrl: './inventory.component.html',
  styleUrls: ['./inventory.component.scss']
})
export class InventoryComponent implements OnInit {
  inventory: InventoryItem[] = [];
  displayedColumns: string[] = ['productName', 'lotNumber', 'expiryDate', 'quantity', 'status'];
  hideExpired: boolean = false;

  constructor(private inventoryService: InventoryService) {}

  ngOnInit(): void {
    this.loadInventory();
  }

  loadInventory(): void {
    this.inventoryService.getAll(this.hideExpired).subscribe(data => {
      this.inventory = data;
    });
  }

  onFilterChange(): void {
    this.loadInventory();
  }
}
