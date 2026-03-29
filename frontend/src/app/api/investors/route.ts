import { NextResponse } from "next/server";
import { INVESTORS } from "./_data";

export async function GET() {
  return NextResponse.json(INVESTORS);
}

