import { api } from "@/lib/api";
import { InvestorCard } from "@/components/InvestorCard";
import { ChangesFeed } from "@/components/ChangesFeed";

export const revalidate = 60;

export default async function HomePage() {
  const [investors, changes] = await Promise.all([
    api.getInvestors().catch(() => []),
    api.getRecentChanges(10).catch(() => []),
  ]);

  return (
    <div className="space-y-10">
      <div>
        <h1 className="text-3xl font-bold text-white">Investor Portfolios</h1>
        <p className="mt-1 text-gray-400">
          Tracking {investors.length} professional investors from Saxo Bank&apos;s Millionaerklubben
        </p>
      </div>

      <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
        {investors.map((investor) => (
          <InvestorCard key={investor.id} investor={investor} />
        ))}
        {investors.length === 0 && (
          <div className="col-span-full rounded-xl border border-gray-800 bg-gray-900 p-12 text-center text-gray-500">
            No investors yet — the backend sync runs on startup. Check the API logs.
          </div>
        )}
      </div>

      {changes.length > 0 && (
        <div>
          <h2 className="mb-4 text-xl font-semibold text-white">Recent Changes</h2>
          <ChangesFeed changes={changes} />
        </div>
      )}
    </div>
  );
}
