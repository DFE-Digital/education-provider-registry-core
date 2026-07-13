-- ------------------------------------------------------------
-- Reference data
-- ------------------------------------------------------------
INSERT INTO ref.establishment_family (code, name) VALUES
('SCH', 'Schools'),
('GRP', 'Groups');

INSERT INTO ref.establishment_type (establishment_family_id, code, name, is_school, is_group)
SELECT establishment_family_id, 'PRI', 'Primary School', TRUE, FALSE
FROM ref.establishment_family WHERE code = 'SCH';

INSERT INTO ref.establishment_type (establishment_family_id, code, name, is_school, is_group)
SELECT establishment_family_id, 'SEC', 'Secondary School', TRUE, FALSE
FROM ref.establishment_family WHERE code = 'SCH';

INSERT INTO ref.establishment_type (establishment_family_id, code, name, is_school, is_group)
SELECT establishment_family_id, 'MAT', 'Multi-Academy Trust', FALSE, TRUE
FROM ref.establishment_family WHERE code = 'GRP';

INSERT INTO ref.establishment_status (code, name) VALUES
('OPEN', 'Open'),
('CLOSED', 'Closed');

INSERT INTO ref.education_phase_group (code, name) VALUES
('PRI', 'Primary'),
('SEC', 'Secondary');

INSERT INTO ref.education_phase (education_phase_group_id, code, name)
SELECT education_phase_group_id, 'KS1', 'Key Stage 1'
FROM ref.education_phase_group WHERE code = 'PRI';

INSERT INTO ref.education_phase (education_phase_group_id, code, name)
SELECT education_phase_group_id, 'KS2', 'Key Stage 2'
FROM ref.education_phase_group WHERE code = 'PRI';

INSERT INTO ref.group_type (code, name) VALUES
('MAT', 'Multi-Academy Trust');

INSERT INTO ref.role_type (code, name) VALUES
('HT', 'Headteacher'),
('CEO', 'Chief Executive Officer'),
('DEP', 'Deputy Headteacher'),
('CHAIR', 'Chair of Governors');

INSERT INTO ref.title (name) VALUES
('Mr'), ('Mrs'), ('Ms'), ('Dr');

INSERT INTO ref.reason_establishment_opened (code, name) VALUES
('NEW', 'Newly Opened');

INSERT INTO ref.reason_establishment_closed (code, name) VALUES
('MERGE', 'Merged');

-- ------------------------------------------------------------
-- People
-- ------------------------------------------------------------
INSERT INTO core.person (title_id, given_name, family_name, display_name)
SELECT
    (SELECT title_id FROM ref.title ORDER BY RANDOM() LIMIT 1),
    'Person' || g,
    'Surname' || g,
    'Person ' || g || ' Surname ' || g
FROM generate_series(1, 200) g;

-- ------------------------------------------------------------
-- Roles
-- ------------------------------------------------------------
INSERT INTO core.role (person_id, role_type_id)
SELECT
    p.person_id,
    (SELECT role_type_id FROM ref.role_type ORDER BY RANDOM() LIMIT 1)
FROM core.person p;

-- ------------------------------------------------------------
-- MATs
-- ------------------------------------------------------------
INSERT INTO core.group_record (code, name, group_type_id)
SELECT
    'MAT' || g,
    'Multi-Academy Trust ' || g,
    (SELECT group_type_id FROM ref.group_type WHERE code = 'MAT')
FROM generate_series(1, 5) g;

-- ------------------------------------------------------------
-- Schools
-- ------------------------------------------------------------
INSERT INTO core.establishment (
    urn, uid, name, establishment_number,
    establishment_type_id, establishment_status_id
)
SELECT
    -- Generate a 5–7 digit numeric URN
    (FLOOR(RANDOM() * 9000000) + 10000)::text,
    'UID' || g,
    'School ' || g,
    LPAD(g::text, 3, '0'),
    (SELECT establishment_type_id FROM ref.establishment_type WHERE code IN ('PRI','SEC') ORDER BY RANDOM() LIMIT 1),
    (SELECT establishment_status_id FROM ref.establishment_status WHERE code = 'OPEN')
FROM generate_series(1, 25) g;


-- ------------------------------------------------------------
-- Assign Schools to MATs
-- ------------------------------------------------------------
INSERT INTO core.establishment_group_membership (establishment_id, group_id, membership_category, start_date)
SELECT
    e.establishment_id,
    g.group_id,
    'Member',
    CURRENT_DATE
FROM core.establishment e
JOIN core.group_record g ON (e.establishment_id % 5) + 1 = g.group_id;

-- ------------------------------------------------------------
-- Role Assignments
-- ------------------------------------------------------------
INSERT INTO core.role_assignment (role_id, establishment_id, group_id, preferred_job_title)
SELECT
    r.role_id,
    CASE 
        WHEN (r.role_id % 5) <> 0 THEN 
            (SELECT establishment_id FROM core.establishment ORDER BY RANDOM() LIMIT 1)
        ELSE NULL
    END,
    CASE 
        WHEN (r.role_id % 5) = 0 THEN 
            (SELECT group_id FROM core.group_record ORDER BY RANDOM() LIMIT 1)
        ELSE NULL
    END,
    'Role ' || r.role_id
FROM core.role r;

-- ------------------------------------------------------------
-- Backfill Headteacher / CEO FKs
-- ------------------------------------------------------------
UPDATE core.establishment e
SET headteacher_role_assignment_id = ra.role_assignment_id
FROM core.role_assignment ra
JOIN core.role r ON r.role_id = ra.role_id
JOIN ref.role_type rt ON rt.role_type_id = r.role_type_id
WHERE rt.code = 'HT'
AND ra.establishment_id = e.establishment_id;

UPDATE core.group_record g
SET headteacher_role_assignment_id = ra.role_assignment_id
FROM core.role_assignment ra
JOIN core.role r ON r.role_id = ra.role_id
JOIN ref.role_type rt ON rt.role_type_id = r.role_type_id
WHERE rt.code = 'CEO'
AND ra.group_id = g.group_id;

-- ------------------------------------------------------------
-- Identifiers
-- ------------------------------------------------------------
INSERT INTO core.establishment_identifier (establishment_id, identifier_type, identifier_value)
SELECT establishment_id, 'UKPRN', '900' || LPAD(establishment_id::text, 5, '0')
FROM core.establishment;

INSERT INTO core.group_identifier (group_id, identifier_type, identifier_value)
SELECT group_id, 'CompaniesHouse', 'CH' || LPAD(group_id::text, 6, '0')
FROM core.group_record;

-- ------------------------------------------------------------
-- Contacts
-- ------------------------------------------------------------
INSERT INTO core.contact (establishment_id, website, telephone_number)
SELECT establishment_id,
       'https://school' || establishment_id || '.example.com',
       '01234 56' || LPAD(establishment_id::text, 2, '0')
FROM core.establishment;

INSERT INTO core.contact (group_id, website, telephone_number)
SELECT group_id,
       'https://mat' || group_id || '.example.com',
       '01999 88' || LPAD(group_id::text, 2, '0')
FROM core.group_record;

-- ------------------------------------------------------------
-- Provision
-- ------------------------------------------------------------
INSERT INTO core.establishment_provision (establishment_id, education_phase_id, fsm, percentage_fsm)
SELECT
    e.establishment_id,
    (SELECT education_phase_id FROM ref.education_phase ORDER BY RANDOM() LIMIT 1),
    (RANDOM() * 200)::int,
    ROUND((RANDOM() * 100)::numeric, 2)
FROM core.establishment e;


-- ------------------------------------------------------------
-- Lifecycle Events
-- ------------------------------------------------------------
INSERT INTO core.establishment_lifecycle_event (establishment_id, event_type, opened_reason_id, event_date)
SELECT
    establishment_id,
    'Opened',
    (SELECT reason_establishment_opened_id FROM ref.reason_establishment_opened WHERE code = 'NEW'),
    CURRENT_DATE - (RANDOM() * 2000)::int
FROM core.establishment;
