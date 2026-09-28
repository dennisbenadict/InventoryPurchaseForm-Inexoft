import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductsComponent } from './features/products/products.component';
import { TaxesComponent } from './features/taxes/taxes.component';
import { SuppliersComponent } from './features/suppliers/suppliers.component';
import { PurchasesComponent } from './features/purchases/purchases.component';
import { PurchaseFormComponent } from './features/purchase-form/purchase-form.component';
import { InventoryComponent } from './features/inventory/inventory.component';

const routes: Routes = [
  { path: 'products', component: ProductsComponent },
  { path: 'taxes', component: TaxesComponent },
  { path: 'suppliers', component: SuppliersComponent },
  { path: 'purchases', component: PurchasesComponent },
  { path: 'purchases/new', component: PurchaseFormComponent },
  { path: 'purchases/:id', component: PurchaseFormComponent },
  { path: 'inventory', component: InventoryComponent },
  { path: '', redirectTo: '/purchases', pathMatch: 'full' },
  { path: '**', redirectTo: '/purchases' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
