import { NextResponse } from "next/server";
import { INVESTORS } from "../route";

export async function GET(_req: Request, { params }: { params: { id: string } }) {
  const investor = INVESTORS.find((i) => i.id === params.id);
  if (!investor) return NextResponse.json({ error: "Not found" }, { status: 404 });
  return NextResponse.json(investor);
}
