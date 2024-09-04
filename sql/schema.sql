CREATE TABLE "AspNetRoles" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Name" VARCHAR(256),
    "NormalizedName" VARCHAR(256),
    "ConcurrencyStamp" TEXT
);

CREATE TABLE "AspNetUsers" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserName" VARCHAR(256),
    "NormalizedUserName" VARCHAR(256),
    "Email" VARCHAR(256),
    "NormalizedEmail" VARCHAR(256),
    "EmailConfirmed" BOOLEAN NOT NULL,
    "PasswordHash" TEXT,
    "SecurityStamp" TEXT,
    "ConcurrencyStamp" TEXT,
    "PhoneNumber" TEXT,
    "PhoneNumberConfirmed" BOOLEAN NOT NULL,
    "TwoFactorEnabled" BOOLEAN NOT NULL,
    "LockoutEnd" TIMESTAMPTZ,
    "LockoutEnabled" BOOLEAN NOT NULL,
    "AccessFailedCount" INT NOT NULL,
    "RefreshToken" VARCHAR(100),
    "RefreshTokenExpiryTime" TIMESTAMPTZ NOT NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL,
    "UpdatedAt" TIMESTAMPTZ NOT NULL
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" SERIAL PRIMARY KEY,
    "RoleId" UUID NOT NULL,
    "ClaimType" TEXT,
    "ClaimValue" TEXT,
    FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" UUID NOT NULL,
    "ClaimType" TEXT,
    "ClaimValue" TEXT,
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT,
    "UserId" UUID NOT NULL,
    PRIMARY KEY ("LoginProvider", "ProviderKey"),
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" UUID NOT NULL,
    "RoleId" UUID NOT NULL,
    PRIMARY KEY ("UserId", "RoleId"),
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" UUID NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT,
    PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Tasks" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Title" VARCHAR(256) NOT NULL,
    "Description" VARCHAR(1024),
    "Status" INTEGER NOT NULL, 
    "Priority" INTEGER NOT NULL, 
    "DueDate" TIMESTAMPTZ NOT NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL,
    "UpdatedAt" TIMESTAMPTZ NOT NULL,
    "UserId" UUID NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Tasks_UserId" ON "Tasks" ("UserId");

CREATE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName") WHERE "NormalizedName" IS NOT NULL;
CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail") WHERE "NormalizedEmail" IS NOT NULL;
CREATE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName") WHERE "NormalizedUserName" IS NOT NULL;
CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");
CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");
CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");
CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

