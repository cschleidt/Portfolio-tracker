import { NextResponse } from "next/server";
const PORTFOLIOS: Record<string, object> = {
  "11111111-0000-0000-0000-000000000001": {
    id: "22222222-0000-0000-0000-000000000001",
    investorId: "11111111-0000-0000-0000-000000000001",
    investorName: "Lars Tvede",
    totalValue: 1342500,
    performancePercent: 34.25,
    lastSyncedAt: new Date().toISOString(),
    holdings: [
      { id: "1", ticker: "NOVO B", companyName: "Novo Nordisk B", quantity: 500, entryPrice: 650, currentPrice: 780, marketValue: 390000, weightPercent: 29.05, changePercent: 20.0, currency: "DKK" },
      { id: "2", ticker: "AAPL", companyName: "Apple Inc.", quantity: 200, entryPrice: 150, currentPrice: 175, marketValue: 245000, weightPercent: 18.25, changePercent: 16.67, currency: "USD" },
      { id: "3", ticker: "MSFT", companyName: "Microsoft Corp.", quantity: 150, entryPrice: 300, currentPrice: 420, marketValue: 434700, weightPercent: 32.38, changePercent: 40.0, currency: "USD" },
      { id: "4", ticker: "NVDA", companyName: "NVIDIA Corp.", quantity: 100, entryPrice: 450, currentPrice: 890, marketValue: 272800, weightPercent: 20.32, changePercent: 97.78, currency: "USD" },
    ],
  },
  "11111111-0000-0000-0000-000000000002": {
    id: "22222222-0000-0000-0000-000000000002",
    investorId: "11111111-0000-0000-0000-000000000002",
    investorName: "Jacob Kirkegaard",
    totalValue: 987600,
    performancePercent: -1.24,
    lastSyncedAt: new Date().toISOString(),
    holdings: [
      { id: "5", ticker: "ORSTED", companyName: "Ørsted A/S", quantity: 300, entryPrice: 500, currentPrice: 380, marketValue: 114000, weightPercent: 11.54, changePercent: -24.0, currency: "DKK" },
      { id: "6", ticker: "DSV", companyName: "DSV A/S", quantity: 100, entryPrice: 1200, currentPrice: 1450, marketValue: 145000, weightPercent: 14.68, changePercent: 20.83, currency: "DKK" },
      { id: "7", ticker: "MAERSK B", companyName: "A.P. Møller-Mærsk B", quantity: 10, entryPrice: 12000, currentPrice: 13500, marketValue: 135000, weightPercent: 13.67, changePercent: 12.5, currency: "DKK" },
      { id: "8", ticker: "GS", companyName: "Goldman Sachs", quantity: 50, entryPrice: 350, currentPrice: 490, marketValue: 593600, weightPercent: 60.11, changePercent: 40.0, currency: "USD" },
    ],
  },
  "11111111-0000-0000-0000-000000000003": {
    id: "22222222-0000-0000-0000-000000000003",
    investorId: "11111111-0000-0000-0000-000000000003",
    investorName: "Anne Buchardt",
    totalValue: 875000,
    performancePercent: -8.75,
    lastSyncedAt: new Date().toISOString(),
    holdings: [
      { id: "9", ticker: "VESTAS", companyName: "Vestas Wind Systems", quantity: 400, entryPrice: 180, currentPrice: 165, marketValue: 66000, weightPercent: 7.54, changePercent: -8.33, currency: "DKK" },
      { id: "10", ticker: "NESTE", companyName: "Neste Oyj", quantity: 300, entryPrice: 40, currentPrice: 35, marketValue: 114450, weightPercent: 13.08, changePercent: -12.5, currency: "EUR" },
      { id: "11", ticker: "TSLA", companyName: "Tesla Inc.", quantity: 100, entryPrice: 200, currentPrice: 175, marketValue: 120750, weightPercent: 13.8, changePercent: -12.5, currency: "USD" },
      { id: "12", ticker: "AMZN", companyName: "Amazon.com Inc.", quantity: 80, entryPrice: 140, currentPrice: 195, marketValue: 573800, weightPercent: 65.58, changePercent: 39.29, currency: "USD" },
    ],
  },
  "11111111-0000-0000-0000-000000000004": {
    id: "22222222-0000-0000-0000-000000000004",
    investorId: "11111111-0000-0000-0000-000000000004",
    investorName: "Peter Nielsen",
    totalValue: 1128000,
    performancePercent: 12.80,
    lastSyncedAt: new Date().toISOString(),
    holdings: [
      { id: "13", ticker: "GOOGL", companyName: "Alphabet Inc.", quantity: 150, entryPrice: 130, currentPrice: 175, marketValue: 393750, weightPercent: 34.91, changePercent: 34.62, currency: "USD" },
      { id: "14", ticker: "META", companyName: "Meta Platforms Inc.", quantity: 200, entryPrice: 300, currentPrice: 525, marketValue: 1575000, weightPercent: 0, changePercent: 75.0, currency: "USD" },
      { id: "15", ticker: "NETCOMPANY", companyName: "Netcompany Group", quantity: 500, entryPrice: 300, currentPrice: 380, marketValue: 190000, weightPercent: 16.84, changePercent: 26.67, currency: "DKK" },
      { id: "16", ticker: "AMBU B", companyName: "Ambu B", quantity: 1000, entryPrice: 150, currentPrice: 185, marketValue: 185000, weightPercent: 16.40, changePercent: 23.33, currency: "DKK" },
    ],
  },
  "11111111-0000-0000-0000-000000000005": {
    id: "22222222-0000-0000-0000-000000000005",
    investorId: "11111111-0000-0000-0000-000000000005",
    investorName: "Mads Christiansen",
    totalValue: 1065000,
    performancePercent: 6.50,
    lastSyncedAt: new Date().toISOString(),
    holdings: [
      { id: "17", ticker: "CARL B", companyName: "Carlsberg B", quantity: 200, entryPrice: 900, currentPrice: 980, marketValue: 196000, weightPercent: 18.40, changePercent: 8.89, currency: "DKK" },
      { id: "18", ticker: "COLO B", companyName: "Coloplast B", quantity: 100, entryPrice: 850, currentPrice: 940, marketValue: 94000, weightPercent: 8.83, changePercent: 10.59, currency: "DKK" },
      { id: "19", ticker: "JYSKE", companyName: "Jyske Bank", quantity: 300, entryPrice: 450, currentPrice: 520, marketValue: 156000, weightPercent: 14.65, changePercent: 15.56, currency: "DKK" },
      { id: "20", ticker: "JPM", companyName: "JPMorgan Chase", quantity: 100, entryPrice: 155, currentPrice: 215, marketValue: 619000, weightPercent: 58.12, changePercent: 38.71, currency: "USD" },
    ],
  },
};

export async function GET(_req: Request, { params }: { params: { id: string } }) {
  const portfolio = PORTFOLIOS[params.id];
  if (!portfolio) return NextResponse.json({ error: "Not found" }, { status: 404 });
  return NextResponse.json(portfolio);
}
