export interface Product {
  productId: number;
  productCode: string;
  productName: string;
  category: string;
  costPrice: number;
  sellingPrice: number;
  requiresLotTracking: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  taxes: Tax[];
  isUsedInTransaction: boolean;
}

export interface ProductCreate {
  productCode: string;
  productName: string;
  category: string;
  costPrice: number;
  sellingPrice: number;
  requiresLotTracking: boolean;
  isActive: boolean;
  taxIds: number[];
}

export interface Tax {
  taxId: number;
  taxName: string;
  percentage: number;
  isCompound: boolean;
  isActive: boolean;
  isDeleted: boolean;
  createdAt: string;
  isUsedInTransaction: boolean;
}

export interface TaxCreate {
  taxName: string;
  percentage: number;
  isCompound: boolean;
  isActive: boolean;
}

export interface Supplier {
  supplierId: number;
  supplierName: string;
  contactInfo: string;
  isActive: boolean;
}

export interface SupplierCreate {
  supplierName: string;
  contactInfo: string;
  isActive: boolean;
}

export interface Purchase {
  purchaseId: number;
  supplierId: number;
  supplierName: string;
  invoiceNumber: string;
  invoiceDate: string;
  totalAmount: number;
  totalTax: number;
  netAmount: number;
  status: string;
  createdAt: string;
  postedAt: string | null;
  items: PurchaseItem[];
}

export interface PurchaseCreate {
  supplierId: number;
  invoiceNumber: string;
  invoiceDate: string;
  items: PurchaseItemCreate[];
}

export interface PurchaseItem {
  purchaseItemId: number;
  productId: number;
  productCode: string;
  productName: string;
  lotNumber: string | null;
  expiryDate: string | null;
  quantity: number;
  rate: number;
  discountPercent: number;
  discountAmount: number;
  lineTotal: number;
  taxAmount: number;
  netAmount: number;
  taxes: PurchaseItemTax[];
}

export interface PurchaseItemCreate {
  productId: number;
  lotNumber: string | null;
  expiryDate: string | null;
  quantity: number;
  rate: number;
  discountPercent: number;
}

export interface PurchaseItemTax {
  taxId: number;
  taxName: string;
  taxPercentage: number;
  isCompound: boolean;
  taxAmount: number;
}

export interface InventoryItem {
  inventoryId: number;
  productId: number;
  productCode: string;
  productName: string;
  lotNumber: string;
  expiryDate: string | null;
  quantity: number;
  isExpired: boolean;
  createdAt: string;
  lastUpdated: string;
}
