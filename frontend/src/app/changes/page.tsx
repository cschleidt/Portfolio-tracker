import { api } from "@/lib/api";
import { ChangesFeed } from "@/components/ChangesFeed";

export const revalidate = 60;

export default async function ChangesPage() {
  const changes = await api.getRecentChanges(100).catch(() => []);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-white">Portfolio Changes</h1>
        <p className="mt-1 text-gray-400">
          Latest detected changes across all investor portfolios
        </p>
      </div>
      <ChangesFeed changes={changes} />
    </div>
  );
}
