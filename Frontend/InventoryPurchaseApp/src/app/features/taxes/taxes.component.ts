import { Component, OnInit } from '@angular/core';
import { TaxService } from '../../core/services/tax.service';
import { Tax, TaxCreate } from '../../core/models/models';
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
  selector: 'app-tax-dialog',
  template: `
    <h2 mat-dialog-title>{{ data ? 'Edit' : 'Add' }} Tax</h2>
    <mat-dialog-content [formGroup]="form">
      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Tax Name</mat-label>
        <input matInput formControlName="taxName" required>
      </mat-form-field>
      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Percentage</mat-label>
        <input matInput type="number" formControlName="percentage" required>
      </mat-form-field>
      <div class="checkbox-group">
        <mat-checkbox formControlName="isCompound">Compound Tax</mat-checkbox>
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
export class TaxDialogComponent {
  form: FormGroup;
  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<TaxDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Tax | null
  ) {
    this.form = this.fb.group({
      taxName: [data?.taxName || '', Validators.required],
      percentage: [data?.percentage || 0, [Validators.required, Validators.min(0)]],
      isCompound: [data ? data.isCompound : false],
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
  selector: 'app-taxes',
  templateUrl: './taxes.component.html',
  styleUrls: ['./taxes.component.scss']
})
export class TaxesComponent implements OnInit {
  taxes: Tax[] = [];
  displayedColumns: string[] = ['taxName', 'percentage', 'isCompound', 'isActive', 'actions'];

  constructor(
    private taxService: TaxService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadTaxes();
  }

  loadTaxes() {
    this.taxService.getAll(true).subscribe(data => this.taxes = data);
  }

  openDialog(tax?: Tax) {
    const dialogRef = this.dialog.open(TaxDialogComponent, {
      width: '400px',
      data: tax || null
    });

    dialogRef.afterClosed().subscribe((result: TaxCreate) => {
      if (result) {
        if (tax) {
          this.taxService.update(tax.taxId, result).subscribe({
            next: () => {
              this.loadTaxes();
              this.snackBar.open('Tax updated', 'Close', { duration: 3000 });
            },
            error: (err) => this.snackBar.open(err.error?.message || 'Error updating tax', 'Close', { duration: 3000 })
          });
        } else {
          this.taxService.create(result).subscribe({
            next: () => {
              this.loadTaxes();
              this.snackBar.open('Tax created', 'Close', { duration: 3000 });
            },
            error: (err) => this.snackBar.open(err.error?.message || 'Error creating tax', 'Close', { duration: 3000 })
          });
        }
      }
    });
  }

  deleteTax(tax: Tax) {
    if (confirm(`Are you sure you want to delete ${tax.taxName}?`)) {
      this.taxService.delete(tax.taxId).subscribe({
        next: () => {
          this.loadTaxes();
          this.snackBar.open('Tax deleted', 'Close', { duration: 3000 });
        },
        error: (err) => this.snackBar.open(err.error?.message || 'Error deleting tax', 'Close', { duration: 3000 })
      });
    }
  }
}
