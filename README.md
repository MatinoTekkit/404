# Projekt - Rezervační systém sportovišť – VPSI

# Rozcestník

- [Zadání](#zadání)
- [1. sprint](#1-sprint)
- [2. sprint](#2-sprint)
- [3. sprint](#3-sprint)
- [4. sprint](#4-sprint)
- [5. sprint](#5-sprint)
- [6. sprint](#6-sprint)
- [7. sprint](#7-sprint)
- [8. sprint](#8-sprint)

---

# `Zadání`

```txt
Rezervační systém pro sportoviště

Cieľ projektu:
    Navrhnúť a implementovať informačný systém umožňujúci rezerváciu športových zariadení (napr. tenisové kurty, telocvične, fitness miestnosti). Systém má umožniť správu zariadení, vytváranie rezervácií a kontrolu dostupnosti v konkrétnom čase.

Projekt má demonštrovať:
- návrh databázového modelu
- implementáciu business logiky
- prácu s používateľskými rolami

Funkčné požiadavky

1. Správa športovísk
- Systém musí umožniť:
- evidenciu športovísk (názov, typ, kapacita, cena za hodinu)
- úpravu údajov
- zobrazenie zoznamu športovísk
- zmenu dostupnosti (napr. mimo prevádzky)
- automatická zľava po 5/10/15 rezerváciach na ďalšiu rezervaciu - 5/10/15%


2. Používatelia
- Systém musí podporovať minimálne 2 role:

Administrátor
- správa športovísk
- prehľad všetkých rezervácií
- zrušenie rezervácie

Zákazník
- vytvorenie rezervácie
- zobrazenie vlastných rezervácií
- zrušenie vlastnej rezervácie
- možnosť registracie/prihlasenia, zmien v profile

3. Rezervácie
- Systém musí umožniť:
- vytvorenie rezervácie (športovisko, dátum, čas od–do)
- kontrolu kolízie rezervácií (nesmie dôjsť k časovému prekrytiu)
- výpočet ceny podľa dĺžky rezervácie

+ požadujeme modern futuristický dizajn celého webu
```

--- 

# `1. sprint`

## Návrh modelu

### Uživatelé

- ADMIN, ZÁKAZNÍK 

- Opravdu jsou jen tyto 2 role? 
```bash

```

- Jméno, email, telefon
- Jaké údaje o nich budeme ukládat?
```bash

```

### Sportoviště

- Jaké sporty umožňujete? (kapacity se můžou lišit: 1 fitko - 40 míst, 4 tenisové kurty - každý s kapacitou 4)
```bash

```

- Je cena sportovišť pevně dána, nebo bude možno udělovat slevy?
```bash

```

### Rezervace

- Jakou formou se bude rezervovat? něco jako rozvrh?
```bash

```

- Budou se provádět rezervace na minuty nebo třeba na každých 15 nebo 30 minut?
- Existuje minimální délka?
```bash

```

- Jak se bude počítat sleva? je na to nějaké pravidlo?
- Je třeba uživatelsky měnit hodnotu slevy?
```bash

```

### Model 

- Zatím máme 3 hlavní entity - Uživatel, Sportoviště, Rezervace
- Vedlejší - Typ sportoviště, Ceník, Odstávka sportoviště, Slevy, 
- Log?
```bash

```

### Provedení
- Nabízíme řešení webové aplikace v C# spolu s databází
- Je to OK?
```bash

```