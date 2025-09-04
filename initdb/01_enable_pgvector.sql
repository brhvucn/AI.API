-- Opret extension (k�rer kun f�rste gang databasen initialiseres)
CREATE EXTENSION IF NOT EXISTS vector;

-- (Valgfrit) Eksempel p� tabel med embedding-felt
-- CREATE TABLE documents (
--   id bigserial PRIMARY KEY,
--   content text NOT NULL,
--   embedding vector(1536)  -- s�t l�ngden til din models dimensions
-- );
--
-- (Valgfrit) Index til hurtigere vektor-s�gning
-- CREATE INDEX ON documents USING ivfflat (embedding vector_l2_ops) WITH (lists = 100);
