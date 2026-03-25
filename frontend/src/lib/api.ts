import type { Investor, Portfolio, PortfolioChange } from "@/types";

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "";

async function fetchJson<T>(path: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${API_URL}/api${path}`, {
    next: { revalidate: 60 },
    ...options,
  });
  if (!res.ok) throw new Error(`API error ${res.status}: ${path}`);
  return res.json();
}

export const api = {
  getInvestors: () => fetchJson<Investor[]>("/investors"),
  getInvestor: (id: string) => fetchJson<Investor>(`/investors/${id}`),
  getPortfolio: (investorId: string) =>
    fetchJson<Portfolio>(`/investors/${investorId}/portfolio`),
  getRecentChanges: (count = 50) =>
    fetchJson<PortfolioChange[]>(`/changes?count=${count}`),
};
