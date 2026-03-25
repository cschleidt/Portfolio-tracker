import { NextResponse } from "next/server";

const CHANGES = [
  {
    id: "c1", portfolioId: "22222222-0000-0000-0000-000000000001",
    investorName: "Lars Tvede", changeType: "HoldingAdded",
    ticker: "NVDA", description: "Lars Tvede har tilføjet NVIDIA Corp. til porteføljen",
    oldValue: null, newValue: 890, detectedAt: new Date(Date.now() - 1000 * 60 * 45).toISOString(),
  },
  {
    id: "c2", portfolioId: "22222222-0000-0000-0000-000000000004",
    investorName: "Peter Nielsen", changeType: "QuantityIncreased",
    ticker: "META", description: "Peter Nielsen har øget beholdningen af Meta Platforms Inc.",
    oldValue: 150, newValue: 200, detectedAt: new Date(Date.now() - 1000 * 60 * 120).toISOString(),
  },
  {
    id: "c3", portfolioId: "22222222-0000-0000-0000-000000000002",
    investorName: "Jacob Kirkegaard", changeType: "HoldingRemoved",
    ticker: "ORSTED", description: "Jacob Kirkegaard overvejer at fjerne Ørsted A/S fra porteføljen",
    oldValue: 380, newValue: null, detectedAt: new Date(Date.now() - 1000 * 60 * 60 * 3).toISOString(),
  },
  {
    id: "c4", portfolioId: "22222222-0000-0000-0000-000000000003",
    investorName: "Anne Buchardt", changeType: "QuantityDecreased",
    ticker: "TSLA", description: "Anne Buchardt har reduceret beholdningen af Tesla Inc.",
    oldValue: 150, newValue: 100, detectedAt: new Date(Date.now() - 1000 * 60 * 60 * 6).toISOString(),
  },
  {
    id: "c5", portfolioId: "22222222-0000-0000-0000-000000000005",
    investorName: "Mads Christiansen", changeType: "HoldingAdded",
    ticker: "JPM", description: "Mads Christiansen har tilføjet JPMorgan Chase til porteføljen",
    oldValue: null, newValue: 215, detectedAt: new Date(Date.now() - 1000 * 60 * 60 * 12).toISOString(),
  },
];

export async function GET(req: Request) {
  const { searchParams } = new URL(req.url);
  const count = parseInt(searchParams.get("count") ?? "50");
  return NextResponse.json(CHANGES.slice(0, count));
}
