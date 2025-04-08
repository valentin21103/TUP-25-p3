#!/bin/bash

# Script para verificar y aceptar solo los Pull Requests que modifican ejercicio.cs en tp2
# Autor: GitHub Copilot
# Fecha: 7 de abril de 2025

# Colores para salida
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${CYAN}===== Verificador de Pull Requests para tp2/ejercicio.cs =====${NC}"
echo ""

# Verificar que estemos en un repositorio Git
if [ ! -d .git ]; then
  echo -e "${RED}Error: Este script debe ejecutarse en la raíz de un repositorio Git.${NC}"
  exit 1
fi

# Asegurarnos de tener la información más reciente
echo -e "${YELLOW}Actualizando información del repositorio...${NC}"
git fetch origin

# Obtener la lista de Pull Requests abiertos usando la API de GitHub
echo -e "${YELLOW}Obteniendo lista de Pull Requests abiertos...${NC}"

# Verificar si gh (GitHub CLI) está instalado
if ! command -v gh &> /dev/null; then
  echo -e "${RED}Error: GitHub CLI (gh) no está instalado.${NC}"
  echo "Puedes instalarlo siguiendo las instrucciones en: https://cli.github.com/"
  echo "O usar el comando 'brew install gh' si estás en macOS y tienes Homebrew."
  exit 1
fi

# Obtener información de Pull Requests usando GitHub CLI
PR_LIST=$(gh pr list --json number,title,headRefName,author --limit 100)

if [ -z "$PR_LIST" ]; then
  echo -e "${YELLOW}No hay Pull Requests abiertos actualmente.${NC}"
  exit 0
fi

# Contar PRs
PR_COUNT=$(echo "$PR_LIST" | jq length)
echo -e "${CYAN}Se encontraron $PR_COUNT Pull Requests abiertos.${NC}"
echo ""

# Array para almacenar PRs válidos
declare -a VALID_PRS

# Revisar cada PR
echo -e "${YELLOW}Analizando cada Pull Request...${NC}"
for (( i=0; i<$PR_COUNT; i++ )); do
  PR_NUM=$(echo "$PR_LIST" | jq -r ".[$i].number")
  PR_TITLE=$(echo "$PR_LIST" | jq -r ".[$i].title")
  PR_AUTHOR=$(echo "$PR_LIST" | jq -r ".[$i].author.login")
  PR_BRANCH=$(echo "$PR_LIST" | jq -r ".[$i].headRefName")
  
  echo -e "${CYAN}PR #$PR_NUM:${NC} $PR_TITLE (por $PR_AUTHOR, rama: $PR_BRANCH)"
  
  # Verificar los archivos modificados en este PR
  FILES_CHANGED=$(gh pr view $PR_NUM --json files | jq -r '.files[].path')
  MODIFIED_EXERCISE=false
  
  for FILE in $FILES_CHANGED; do
    if [[ $FILE =~ tp2/ejercicio\.cs$ ]]; then
      echo -e "  ${GREEN}✓ Modifica tp2/ejercicio.cs:${NC} $FILE"
      MODIFIED_EXERCISE=true
    fi
  done
  
  if [ "$MODIFIED_EXERCISE" = true ]; then
    echo -e "  ${GREEN}✓ Este PR cumple con el criterio para ser aceptado.${NC}"
    VALID_PRS+=($PR_NUM)
  else
    echo -e "  ${RED}✗ Este PR NO modifica ningún archivo tp2/ejercicio.cs${NC}"
  fi
  echo ""
done

# Mostrar resumen
echo -e "${CYAN}===== Resumen =====${NC}"
if [ ${#VALID_PRS[@]} -eq 0 ]; then
  echo -e "${YELLOW}No hay Pull Requests que cumplan con el criterio.${NC}"
  exit 0
fi

echo -e "${GREEN}Pull Requests que cumplen con el criterio (${#VALID_PRS[@]}):${NC}"
for PR in "${VALID_PRS[@]}"; do
  PR_TITLE=$(echo "$PR_LIST" | jq -r ".[] | select(.number == $PR) | .title")
  echo -e "${GREEN}PR #$PR:${NC} $PR_TITLE"
done
echo ""

# Preguntar si desea aceptar los PRs válidos
echo -e "${YELLOW}¿Deseas aceptar todos los Pull Requests válidos? (s/n):${NC}"
read -r ANSWER

if [[ $ANSWER =~ ^[Ss]$ ]]; then
  for PR in "${VALID_PRS[@]}"; do
    echo -e "${CYAN}Aceptando PR #$PR...${NC}"
    gh pr merge $PR --merge --delete-branch
  done
  echo -e "${GREEN}¡Proceso completado!${NC}"
else
  echo -e "${YELLOW}Para aceptar un PR específico, usa:${NC}"
  echo "gh pr merge [número-PR] --merge --delete-branch"
  echo ""
  echo -e "${YELLOW}Para revisar un PR específico:${NC}"
  echo "gh pr checkout [número-PR]"
fi