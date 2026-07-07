import { UserButton } from "@clerk/react";
import { BarChart3, House, NotebookPen } from "lucide-react";
import { NavLink, Outlet } from "react-router";
import { cn } from "@/lib/utils";

const tabs = [
  { to: "/", label: "Dashboard", icon: House },
  { to: "/sessions", label: "Sessions", icon: NotebookPen },
  { to: "/analytics", label: "Analytics", icon: BarChart3 },
] as const;

export function AppLayout() {
  return (
    <div className="min-h-screen bg-background text-foreground">
      <header className="sticky top-0 z-20 border-b border-border bg-background/95 px-4 py-3 backdrop-blur">
        <div className="mx-auto flex w-full max-w-3xl items-center justify-between">
          <h1 className="text-lg font-semibold">Basketball Progress Tracker</h1>
          <UserButton />
        </div>
      </header>

      <main className="mx-auto w-full max-w-3xl px-4 pb-24 pt-4">
        <Outlet />
      </main>

      <nav className="fixed inset-x-0 bottom-0 z-30 border-t border-border bg-background/95 backdrop-blur">
        <div className="mx-auto grid w-full max-w-3xl grid-cols-3">
          {tabs.map((tab) => (
            <NavLink
              key={tab.to}
              to={tab.to}
              className={({ isActive }) =>
                cn(
                  "flex flex-col items-center justify-center gap-1 px-2 py-3 text-xs font-medium transition-colors",
                  isActive
                    ? "text-primary"
                    : "text-muted-foreground hover:text-foreground",
                )
              }
            >
              <tab.icon className="size-4" />
              <span>{tab.label}</span>
            </NavLink>
          ))}
        </div>
      </nav>
    </div>
  );
}
