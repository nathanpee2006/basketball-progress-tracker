import { SignInButton, SignUpButton } from "@clerk/react";
import {
  BarChart3,
  Flame,
  NotebookPen,
  Podium,
  Trophy,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { ModeToggle } from "@/components/ui/mode-toggle";

const features = [
  {
    icon: Flame,
    title: "Weekly Streaks",
    description:
      "Stay consistent with streak tracking.",
  },
  {
    icon: NotebookPen,
    title: "Session Logging",
    description:
      "Log zone shooting, free throws, and drills with an interactive court.",
  },
  {
    icon: BarChart3,
    title: "Shooting Analytics",
    description:
      "Spot trends in your weakest zones, free-throw percentage, and volume.",
  },
  {
    icon: Podium,
    title: "Leaderboard",
    description: "Compare your shooting performance against other players.",
  },
  {
    icon: Trophy,
    title: "Achievements",
    description:
      "Unlock milestones as you hit training goals and build habits.",
  },
] as const;

export function LandingPage() {
  const unsafeMetadata: SignUpUnsafeMetadata = {
    timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone,
  };

  return (
    <div className="min-h-screen bg-background text-foreground">
      <header className="sticky top-0 z-20 border-b border-border bg-background/95 px-4 py-3 backdrop-blur">
        <div className="mx-auto flex w-full max-w-3xl items-center justify-between">
          <span className="text-sm font-semibold">Basketball Progress Tracker</span>
          <ModeToggle />
        </div>
      </header>

      <main className="mx-auto w-full max-w-3xl px-4 pb-16 pt-10">
        <section className="space-y-6 text-center">
          <div className="space-y-3">
            <h1 className="text-3xl font-semibold tracking-tight sm:text-4xl">
              Track your game. Build your streak.
            </h1>
            <p className="mx-auto max-w-lg text-sm text-muted-foreground sm:text-base">
              Log practice sessions, monitor weekly consistency, and unlock
              insights that help you improve shot by shot.
            </p>
          </div>

          <div className="flex flex-col items-center justify-center gap-3 sm:flex-row">
            <SignUpButton mode="modal" unsafeMetadata={unsafeMetadata}>
              <Button
                size="lg"
                className="w-full bg-orange-500 hover:bg-orange-600 sm:w-auto"
              >
                Get Started
              </Button>
            </SignUpButton>
            <SignInButton>
              <Button variant="outline" size="lg" className="w-full sm:w-auto">
                Sign In
              </Button>
            </SignInButton>
          </div>
        </section>

        <section className="mt-12 grid gap-4 sm:grid-cols-2">
          {features.map((feature) => (
            <Card key={feature.title}>
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-base">
                  <feature.icon className="size-4 text-orange-500" />
                  {feature.title}
                </CardTitle>
                <CardDescription>{feature.description}</CardDescription>
              </CardHeader>
            </Card>
          ))}
        </section>
      </main>
    </div>
  );
}
