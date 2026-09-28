import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PurchaseService } from '../../core/services/purchase.service';
import { SupplierService } from '../../core/services/supplier.service';
import { ProductService } from '../../core/services/product.service';
import { Supplier, Product, PurchaseCreate } from '../../core/models/models';

@Component({
  selector: 'app-purchase-form',
  templateUrl: './purchase-form.component.html',
  styleUrls: ['./purchase-form.component.scss']
})
export class PurchaseFormComponent implements OnInit {
  purchaseForm: FormGroup;
  suppliers: Supplier[] = [];
  products: Product[] = [];
  purchaseId: number | null = null;
  isPosted: boolean = false;
  netAmount: number = 0;
  totalAmount: number = 0;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private purchaseService: PurchaseService,
    private supplierService: SupplierService,
    private productService: ProductService
  ) {
    this.purchaseForm = this.fb.group({
      supplierId: ['', Validators.required],
      invoiceNumber: ['', Validators.required],
      invoiceDate: ['', Validators.required],
      items: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.loadSuppliers();
    this.loadProducts();

    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id && id !== 'new') {
        this.purchaseId = +id;
        this.loadPurchase(this.purchaseId);
      } else {
        this.addItem(); // add one empty item initially
      }
    });

    this.purchaseForm.valueChanges.subscribe(() => {
      this.calculateTotals();
    });
  }

  get items(): FormArray {
    return this.purchaseForm.get('items') as FormArray;
  }

  loadSuppliers(): void {
    this.supplierService.getAll().subscribe(data => {
      this.suppliers = data;
    });
  }

  loadProducts(): void {
    this.productService.getAll().subscribe(data => {
      this.products = data;
    });
  }

  loadPurchase(id: number): void {
    this.purchaseService.getById(id).subscribe(purchase => {
      this.purchaseForm.patchValue({
        supplierId: purchase.supplierId,
        invoiceNumber: purchase.invoiceNumber,
        invoiceDate: purchase.invoiceDate ? purchase.invoiceDate.substring(0, 10) : ''
      });

      this.isPosted = purchase.status === 'Posted';

      purchase.items.forEach(item => {
        this.items.push(this.fb.group({
          productId: [item.productId, Validators.required],
          lotNumber: [item.lotNumber],
          expiryDate: [item.expiryDate ? item.expiryDate.substring(0, 10) : ''],
          quantity: [item.quantity, [Validators.required, Validators.min(1)]],
          rate: [item.rate, [Validators.required, Validators.min(0)]],
          discountPercent: [item.discountPercent, [Validators.min(0), Validators.max(100)]]
        }));
      });

      if (this.isPosted) {
        this.purchaseForm.disable();
      }
      
      this.calculateTotals();
    });
  }

  addItem(): void {
    if (this.isPosted) return;
    this.items.push(this.fb.group({
      productId: ['', Validators.required],
      lotNumber: [''],
      expiryDate: [''],
      quantity: [1, [Validators.required, Validators.min(1)]],
      rate: [0, [Validators.required, Validators.min(0)]],
      discountPercent: [0, [Validators.min(0), Validators.max(100)]]
    }));
  }

  removeItem(index: number): void {
    if (this.isPosted) return;
    this.items.removeAt(index);
  }

  onProductChange(index: number): void {
    const itemGroup = this.items.at(index);
    const productId = itemGroup.get('productId')?.value;
    const product = this.products.find(p => p.productId === productId);
    if (product) {
      itemGroup.patchValue({
        rate: product.costPrice
      });
    }
  }

  calculateTotals(): void {
    let tAmount = 0;
    let nAmount = 0;
    
    this.items.controls.forEach(control => {
      const qty = control.get('quantity')?.value || 0;
      const rate = control.get('rate')?.value || 0;
      const discount = control.get('discountPercent')?.value || 0;
      
      const lineTotal = qty * rate;
      const discountAmt = lineTotal * (discount / 100);
      
      tAmount += lineTotal;
      nAmount += (lineTotal - discountAmt);
    });
    
    this.totalAmount = tAmount;
    this.netAmount = nAmount;
  }

  save(): void {
    if (this.purchaseForm.invalid) {
      this.purchaseForm.markAllAsTouched();
      return;
    }

    const payload: PurchaseCreate = this.purchaseForm.value;
    
    if (this.purchaseId) {
      this.purchaseService.update(this.purchaseId, payload).subscribe(() => {
        this.router.navigate(['/purchases']);
      });
    } else {
      this.purchaseService.create(payload).subscribe(() => {
        this.router.navigate(['/purchases']);
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/purchases']);
  }
}
