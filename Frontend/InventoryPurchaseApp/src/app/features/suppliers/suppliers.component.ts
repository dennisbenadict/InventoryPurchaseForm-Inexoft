import { Component, OnInit } from '@angular/core';
import { SupplierService } from '../../core/services/supplier.service';
import { Supplier, SupplierCreate } from '../../core/models/models';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-supplier-dialog',
  template: `
    <h2 mat-dialog-title>{{ data ? 'Edit' : 'Add' }} Supplier</h2>
    <mat-dialog-content [formGroup]="form">
      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Supplier Name</mat-label>
        <input matInput formControlName="supplierName" required>
      </mat-form-field>
      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Contact Info</mat-label>
        <input matInput formControlName="contactInfo">
      </mat-form-field>
      <div class="checkbox-group">
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
  imports: [CommonModule, ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatCheckboxModule, MatDialogModule]
})
export class SupplierDialogComponent {
  form: FormGroup;
  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<SupplierDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Supplier | null
  ) {
    this.form = this.fb.group({
      supplierName: [data?.supplierName || '', Validators.required],
      contactInfo: [data?.contactInfo || ''],
      isActive: [data ? data.isActive : true]
    });
  }

  save() {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }
}

@Component({
  selector: 'app-suppliers',
  templateUrl: './suppliers.component.html',
  styleUrls: ['./suppliers.component.scss']
})
export class SuppliersComponent implements OnInit {
  suppliers: Supplier[] = [];
  displayedColumns: string[] = ['supplierName', 'contactInfo', 'isActive', 'actions'];

  constructor(
    private supplierService: SupplierService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadSuppliers();
  }

  loadSuppliers() {
    this.supplierService.getAll().subscribe(data => this.suppliers = data);
  }

  openDialog(supplier?: Supplier) {
    const dialogRef = this.dialog.open(SupplierDialogComponent, {
      width: '400px',
      data: supplier || null
    });

    dialogRef.afterClosed().subscribe((result: SupplierCreate) => {
      if (result) {
        if (supplier) {
          this.supplierService.update(supplier.supplierId, result).subscribe({
            next: () => {
              this.loadSuppliers();
              this.snackBar.open('Supplier updated', 'Close', { duration: 3000 });
            },
            error: (err) => this.snackBar.open(err.error?.message || 'Error updating supplier', 'Close', { duration: 3000 })
          });
        } else {
          this.supplierService.create(result).subscribe({
            next: () => {
              this.loadSuppliers();
              this.snackBar.open('Supplier created', 'Close', { duration: 3000 });
            },
            error: (err) => this.snackBar.open(err.error?.message || 'Error creating supplier', 'Close', { duration: 3000 })
          });
        }
      }
    });
  }
}
