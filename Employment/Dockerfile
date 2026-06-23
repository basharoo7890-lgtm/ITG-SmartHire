# ============================================================
# Dockerfile — PRODUCTION (Multi-stage build)
# ============================================================
# Stage 1: Restore dependencies (cached separately for speed)
# Stage 2: Build & Publish
# Stage 3: Final runtime image (minimal, secure)
# ============================================================

# ---- Stage 1: Restore ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src

# Copy only the project file first (Docker layer cache optimization)
COPY Employment.csproj ./
RUN dotnet restore Employment.csproj --runtime linux-x64

# ---- Stage 2: Build & Publish ----
FROM restore AS publish
WORKDIR /src

# Copy the full source
COPY . .

# Publish as self-contained, single-folder, Release mode
RUN dotnet publish Employment.csproj \
    --configuration Release \
    --runtime linux-x64 \
    --self-contained false \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ---- Stage 3: Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Security: Run as non-root user
RUN addgroup --system --gid 1001 appgroup \
    && adduser --system --uid 1001 --ingroup appgroup appuser

# Create uploads directory and set ownership
# In production, mount this as a volume so CVs persist across deployments
RUN mkdir -p /app/wwwroot/uploads \
    && chown -R appuser:appgroup /app

# Copy published output from publish stage
COPY --from=publish --chown=appuser:appgroup /app/publish .

# Switch to non-root user
USER appuser

# Expose HTTP port (HTTPS is handled by the reverse proxy: Nginx / Azure / AWS ALB)
EXPOSE 8080

# Health check — ensures the container is healthy before receiving traffic
HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:8080/ 2>/dev/null || exit 1

# Set the environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "Employment.dll"]
