// scripts/ci/validate-ci-deps.mjs
import fs from "node:fs";
import { fileURLToPath } from "node:url";
import path from "node:path";
import process from "node:process";
import yaml from "js-yaml";

function fail(message) {
    console.error(message);
    process.exit(1);
}

console.log("Validating CI dependency graph...");

// ------------------------------------------------------------
// Resolve repo root
// ------------------------------------------------------------
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// repo root = two levels up from scripts/ci/
const repoRoot = path.resolve(__dirname, "..", "..");

// ------------------------------------------------------------
// Load files
// ------------------------------------------------------------
const depsPath = path.join(repoRoot, ".github", "ci", "ci-deps.json");
const workflowPath = path.join(repoRoot, ".github", "workflows", "ci.yaml");

if (!fs.existsSync(depsPath)) fail(`ci-deps.json not found at ${depsPath}`);
if (!fs.existsSync(workflowPath)) fail(`ci.yaml not found at ${workflowPath}`);

const deps = JSON.parse(fs.readFileSync(depsPath, "utf8"));
const rawWorkflow = fs.readFileSync(workflowPath, "utf8");
const workflow = yaml.load(rawWorkflow);

if (!workflow.jobs) fail("Workflow contains no jobs.");

const jobs = Object.keys(workflow.jobs);
const orchestrationJobs = ["determine-changes", "validate-ci-deps"];

// ------------------------------------------------------------
// 1. Validate job existence
// ------------------------------------------------------------
for (const zone of Object.keys(deps)) {
    if (!jobs.includes(zone)) {
        fail(`Workflow is missing required job: ${zone}`);
    }
}

// ------------------------------------------------------------
// 2. Validate dependencies match exactly
// ------------------------------------------------------------
for (const zone of Object.keys(deps)) {
    const expectedDeps = deps[zone] || [];
    const job = workflow.jobs[zone];

    let actualDeps = job.needs || [];
    if (typeof actualDeps === "string") actualDeps = [actualDeps];

    const missing = expectedDeps.filter((d) => !actualDeps.includes(d));
    if (missing.length > 0) {
        fail(`Job '${zone}' is missing required dependencies: ${missing.join(", ")}`);
    }

    const extra = actualDeps.filter(
        (d) => !expectedDeps.includes(d) && !orchestrationJobs.includes(d)
    );
    if (extra.length > 0) {
        fail(`Job '${zone}' has extra dependencies not in ci-deps.json: ${extra.join(", ")}`);
    }
}

// ------------------------------------------------------------
// 3. Validate determine-changes + paths-filter
// ------------------------------------------------------------
const determine = workflow.jobs["determine-changes"];
if (!determine || !determine.steps) {
    fail("determine-changes job missing steps.");
}

const hasPathsFilter = determine.steps.some(
    (s) => typeof s.uses === "string" && s.uses.includes("dorny/paths-filter")
);

if (!hasPathsFilter) {
    fail("determine-changes job missing dorny/paths-filter step.");
}

// ------------------------------------------------------------
// 4. Validate nightly schedule
// ------------------------------------------------------------
const onSection = workflow.on;
if (!onSection || !onSection.schedule) {
    fail("Workflow missing nightly schedule trigger.");
}

const schedule = Array.isArray(onSection.schedule)
    ? onSection.schedule
    : [onSection.schedule];

const hasNightly = schedule.some((s) => s && s.cron === "0 9 * * *");

if (!hasNightly) {
    fail("Workflow missing required nightly full run at 09:00 UTC.");
}

// ------------------------------------------------------------
// 5. Validate workflow_dispatch exists
// ------------------------------------------------------------
const onKeys = Object.keys(onSection);
if (!onKeys.includes("workflow_dispatch")) {
    fail("Workflow missing workflow_dispatch manual trigger.");
}

// ------------------------------------------------------------
// 6. Validate determine-changes outputs are referenced
// ------------------------------------------------------------
for (const zone of Object.keys(deps)) {
    for (const dep of deps[zone]) {
        const pattern = new RegExp(`needs\\.determine-changes\\.outputs\\.${dep}`);
        if (!pattern.test(rawWorkflow)) {
            fail(
                `Workflow missing determine-changes output reference for dependency: ${zone} → ${dep}`
            );
        }
    }
}

// ------------------------------------------------------------
// 7. Validate no extra jobs exist
// ------------------------------------------------------------
const allowedJobs = new Set([...orchestrationJobs, ...Object.keys(deps)]);
const extraJobs = jobs.filter((j) => !allowedJobs.has(j));

if (extraJobs.length > 0) {
    fail(
        `Workflow contains extra jobs not defined in ci-deps.json: ${extraJobs.join(", ")}`
    );
}

console.log("CI dependency graph is valid.");
process.exit(0);
