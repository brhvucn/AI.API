-- Opret extension (kører kun første gang databasen initialiseres)
CREATE EXTENSION IF NOT EXISTS vector;

-- (Valgfrit) Eksempel på tabel med embedding-felt
-- CREATE TABLE documents (
--   id bigserial PRIMARY KEY,
--   content text NOT NULL,
--   embedding vector(1536)  -- sæt længden til din models dimensions
-- );
--
-- (Valgfrit) Index til hurtigere vektor-søgning
-- CREATE INDEX ON documents USING ivfflat (embedding vector_l2_ops) WITH (lists = 100);
