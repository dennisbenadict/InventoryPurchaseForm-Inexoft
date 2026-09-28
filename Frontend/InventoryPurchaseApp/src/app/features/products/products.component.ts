import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../core/services/product.service';
import { TaxService } from '../../core/services/tax.service';
import { Product, ProductCreate, Tax } from '../../core/models/models';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-product-dialog',
  template: `
    <h2 mat-dialog-title>{{ data.product ? 'Edit' : 'Add' }} Product</h2>
    <mat-dialog-content [formGroup]="form">
      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Product Code</mat-label>
        <input matInput formControlName="productCode" required>
      </mat-form-field>
      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Product Name</mat-label>
        <input matInput formControlName="productName" required>
      </mat-form-field>
      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Category</mat-label>
        <input matInput formControlName="category">
      </mat-form-field>
      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Cost Price</mat-label>
        <input matInput type="number" formControlName="costPrice" required>
      </mat-form-field>
      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Selling Price</mat-label>
        <input matInput type="number" formControlName="sellingPrice" required>
      </mat-form-field>
      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Taxes</mat-label>
        <mat-select formControlName="taxIds" multiple>
          <mat-option *ngFor="let tax of data.taxes" [value]="tax.taxId">
            {{ tax.taxName }} ({{ tax.percentage }}%)
          </mat-option>
        </mat-select>
      </mat-form-field>
      <div class="checkbox-group">
        <mat-checkbox formControlName="requiresLotTracking">Requires Lot Tracking</mat-checkbox>
        <mat-checkbox formControlName="isActive">Active</mat-checkbox>
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-raised-button color="primary" [disabled]="form.invalid" (click)="save()">Save</button>
    </mat-dialog-actions>
  `,
  styles: [`
    .full-width { width: 100%; margin-bottom: 10px; }
    .checkbox-group { display: flex; flex-direction: column; gap: 10px; margin-bottom: 10px; }
  `],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatCheckboxModule, MatSelectModule, MatDialogModule]
})
export class ProductDialogComponent {
  form: FormGroup;
  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<ProductDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { product: Product | null, taxes: Tax[] }
  ) {
    const p = data.product;
    this.form = this.fb.group({
      productCode: [p?.productCode || '', Validators.required],
      productName: [p?.productName || '', Validators.required],
      category: [p?.category || ''],
      costPrice: [p?.costPrice || 0, [Validators.required, Validators.min(0)]],
      sellingPrice: [p?.sellingPrice || 0, [Validators.required, Validators.min(0)]],
      requiresLotTracking: [p ? p.requiresLotTracking : false],
      isActive: [p ? p.isActive : true],
      taxIds: [p?.taxes?.map(t => t.taxId) || []]
    });
  }

  save() {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }
}

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnInit {
  products: Product[] = [];
  availableTaxes: Tax[] = [];
  displayedColumns: string[] = ['productCode', 'productName', 'category', 'costPrice', 'sellingPrice', 'requiresLotTracking', 'isActive', 'actions'];

  constructor(
    private productService: ProductService,
    private taxService: TaxService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadTaxes();
  }

  loadProducts() {
    this.productService.getAll().subscribe(data => this.products = data);
  }

  loadTaxes() {
    this.taxService.getAll(false).subscribe(data => this.availableTaxes = data);
  }

  openDialog(product?: Product) {
    const dialogRef = this.dialog.open(ProductDialogComponent, {
      width: '500px',
      data: { product: product || null, taxes: this.availableTaxes }
    });

    dialogRef.afterClosed().subscribe((result: ProductCreate) => {
      if (result) {
        if (product) {
          this.productService.update(product.productId, result).subscribe({
            next: () => {
              this.loadProducts();
              this.snackBar.open('Product updated', 'Close', { duration: 3000 });
            },
            error: (err) => this.snackBar.open(err.error?.message || 'Error updating product', 'Close', { duration: 3000 })
          });
        } else {
          this.productService.create(result).subscribe({
            next: () => {
              this.loadProducts();
              this.snackBar.open('Product created', 'Close', { duration: 3000 });
            },
            error: (err) => this.snackBar.open(err.error?.message || 'Error creating product', 'Close', { duration: 3000 })
          });
        }
      }
    });
  }

  deleteProduct(product: Product) {
    if (confirm(`Are you sure you want to delete ${product.productName}?`)) {
      this.productService.delete(product.productId).subscribe({
        next: () => {
          this.loadProducts();
          this.snackBar.open('Product deleted', 'Close', { duration: 3000 });
        },
        error: (err) => this.snackBar.open(err.error?.message || 'Error deleting product. It might be in use.', 'Close', { duration: 3000 })
      });
    }
  }
}
