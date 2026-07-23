import { useForm, useFieldArray } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useParams, useNavigate } from "react-router";
import { useSession } from "../sessions-detail/useSession";
import { useUpdateSession } from "./useUpdateSession";
import {
  sessionFormSchema,
  type SessionFormValues,
} from "./schemas/sessionFormSchema";
import { ZoneInputs } from "./components/ZoneInputs";
import { FreeThrowInputs } from "./components/FreeThrowInputs";
import { Button } from "@/components/ui/button";
import { DrillListEditor } from "./components/DrillListEditor";
import { DatePickerField } from "./components/DatePickerField";
import { CourtVisualization } from "@/components/session/CourtVisualization";
import { zoneFieldsToZoneStats, type ZoneId } from "@/types/court";
import { useCreateSession } from "./useCreateSession";
import { toast } from "sonner";
import { Spinner } from "@/components/ui/spinner";
import { SessionNotFound } from "../../components/session/SessionNotFound";
import { SessionDetailSkeleton } from "../../components/session/SessionDetailSkeleton";

type SessionFormPageProps = {
  mode: "create" | "edit";
};

const defaultValues: SessionFormValues = {
  date: "",
  paintMakes: 0,
  paintAttempts: 0,
  midrangeMakes: 0,
  midrangeAttempts: 0,
  threePointMakes: 0,
  threePointAttempts: 0,
  freeThrowMakes: 0,
  freeThrowAttempts: 0,
  drills: [],
};

export function SessionFormPage({ mode }: SessionFormPageProps) {
  const { id } = useParams();
  const navigate = useNavigate();
  const { session, isLoading: isSessionLoading } = useSession(Number(id));

  const formValues =
    mode === "edit" && session
      ? {
          date: session.date,
          paintMakes: session.paintMakes,
          paintAttempts: session.paintAttempts,
          midrangeMakes: session.midrangeMakes,
          midrangeAttempts: session.midrangeAttempts,
          threePointMakes: session.threePointMakes,
          threePointAttempts: session.threePointAttempts,
          freeThrowMakes: session.freeThrowMakes,
          freeThrowAttempts: session.freeThrowAttempts,
          drills: session.drills.map((drill) => ({
            name: drill.name,
            completionTimeInSeconds: drill.completionTimeInSeconds,
          })),
        }
      : defaultValues;

  const form = useForm<SessionFormValues>({
    resolver: zodResolver(sessionFormSchema),
    defaultValues,
    values: formValues,
  });

  const {
    register,
    control,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
  } = form;

  const watchedZoneFields = watch([
    "paintMakes",
    "paintAttempts",
    "midrangeMakes",
    "midrangeAttempts",
    "threePointMakes",
    "threePointAttempts",
    "freeThrowMakes",
    "freeThrowAttempts",
  ]);

  const zones = zoneFieldsToZoneStats({
    paintMakes: watchedZoneFields[0],
    paintAttempts: watchedZoneFields[1],
    midrangeMakes: watchedZoneFields[2],
    midrangeAttempts: watchedZoneFields[3],
    threePointMakes: watchedZoneFields[4],
    threePointAttempts: watchedZoneFields[5],
    freeThrowMakes: watchedZoneFields[6],
    freeThrowAttempts: watchedZoneFields[7],
  });

  const handleZoneChange = (
    zoneId: ZoneId,
    makes: number,
    attempts: number,
  ) => {
    const fieldMap: Record<
      ZoneId,
      [keyof SessionFormValues, keyof SessionFormValues]
    > = {
      paint: ["paintMakes", "paintAttempts"],
      midrange: ["midrangeMakes", "midrangeAttempts"],
      threePoint: ["threePointMakes", "threePointAttempts"],
      freeThrow: ["freeThrowMakes", "freeThrowAttempts"],
    };
    const [makesField, attemptsField] = fieldMap[zoneId];
    setValue(makesField, makes, { shouldValidate: true, shouldDirty: true });
    setValue(attemptsField, attempts, {
      shouldValidate: true,
      shouldDirty: true,
    });
  };

  const drillFieldArray = useFieldArray({ control, name: "drills" });

  const { createSession, isLoading: isCreating } = useCreateSession();
  const { updateSession, isLoading: isUpdating } = useUpdateSession();

  const onSubmit = async (data: SessionFormValues) => {
    if (mode === "create") {
      try {
        await createSession(data);
        navigate("/sessions");
        toast.success("Session created successfully!");
      } catch (error) {
        toast.error(`Failed to create session.`);
      }
    }
    if (mode === "edit" && id) {
      try {
        await updateSession(Number(id), data);
        navigate(`/sessions/${id}`);
        toast.success("Session updated successfully!");
      } catch (error) {
        toast.error(`Failed to update session.`);
      }
    }
  };

  if (isSessionLoading) {
    return <SessionDetailSkeleton />;
  } else if (mode === "edit" && !session && !isSessionLoading) {
    return <SessionNotFound onBackToSessions={() => navigate("/sessions")} />;
  }

  return (
    <section className="space-y-2">
      <h2 className="text-xl font-semibold">
        {mode === "create" ? "New Session" : "Edit Session"}
      </h2>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        <CourtVisualization
          mode="edit"
          zones={zones}
          onZoneChange={handleZoneChange}
        />
        <DatePickerField control={control} />
        <ZoneInputs register={register} errors={errors} />
        <FreeThrowInputs register={register} errors={errors} />
        <DrillListEditor
          register={register}
          fieldArray={drillFieldArray}
          errors={errors}
        />

        <div className="flex gap-3">
          <Button type="submit" disabled={isCreating || isUpdating}>
            <Spinner
              className={`mr-2 h-4 w-4 animate-spin ${isCreating || isUpdating ? "inline-block" : "hidden"}`}
              data-icon="inline-start"
            />
            {isCreating || isUpdating ? "Saving..." : "Save"}
          </Button>
          <Button
            type="button"
            variant="destructive"
            onClick={() => navigate("/sessions")}
          >
            Cancel
          </Button>
        </div>
      </form>
    </section>
  );
}
