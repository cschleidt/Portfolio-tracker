export interface Investor {
  id: string;
  name: string;
  description: string;
  imageUrl: string | null;
  totalPortfolioValue: number;
  performancePercent: number;
  lastSyncedAt: string;
  holdingsCount: number;
}

export interface Holding {
  id: string;
  ticker: string;
  companyName: string;
  quantity: number;
  entryPrice: number;
  currentPrice: number;
  marketValue: number;
  weightPercent: number;
  changePercent: number;
  currency: string;
}

export interface Portfolio {
  id: string;
  investorId: string;
  investorName: string;
  totalValue: number;
  performancePercent: number;
  lastSyncedAt: string;
  holdings: Holding[];
}

export type ChangeType =
  | "HoldingAdded"
  | "HoldingRemoved"
  | "QuantityIncreased"
  | "QuantityDecreased"
  | "PerformanceUpdated";

export interface PortfolioChange {
  id: string;
  portfolioId: string;
  investorName: string;
  changeType: ChangeType;
  ticker: string;
  description: string;
  oldValue: number | null;
  newValue: number | null;
  detectedAt: string;
}
